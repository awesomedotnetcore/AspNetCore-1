// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2
{
    public class Http2MessageBody : MessageBody
    {
        private readonly Http2Stream _context;
        private ReadResult _readResult;

        private Http2MessageBody(Http2Stream context, MinDataRate minRequestBodyDataRate)
            : base(context, minRequestBodyDataRate)
        {
            _context = context;
        }

        protected override void OnReadStarting()
        {
            // Note ContentLength or MaxRequestBodySize may be null
            if (_context.RequestHeaders.ContentLength > _context.MaxRequestBodySize)
            {
                BadHttpRequestException.Throw(RequestRejectionReason.RequestBodyTooLarge);
            }
        }

        protected override void OnReadStarted()
        {
            // Produce 100-continue if no request body data for the stream has arrived yet.
            if (!_context.RequestBodyStarted)
            {
                TryProduceContinue();
            }
        }

        protected override void OnDataRead(long bytesRead)
        {
            // The HTTP/2 flow control window cannot be larger than 2^31-1 which limits bytesRead.
            _context.OnDataRead((int)bytesRead);
            AddAndCheckConsumedBytes(bytesRead);
        }

        public static MessageBody For(Http2Stream context, MinDataRate minRequestBodyDataRate)
        {
            if (context.ReceivedEmptyRequestBody)
            {
                return ZeroContentLengthClose;
            }

            return new Http2MessageBody(context, minRequestBodyDataRate);
        }

        public override void AdvanceTo(SequencePosition consumed)
        {
            AdvanceTo(consumed, consumed);
        }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            var dataLength = _readResult.Buffer.Slice(_readResult.Buffer.Start, consumed).Length;
            _context.RequestPipe.Reader.AdvanceTo(consumed, examined);
            OnDataRead(dataLength);
        }

        public override bool TryRead(out ReadResult readResult)
        {
            return _context.RequestPipe.Reader.TryRead(out readResult);
        }

        public override async ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            TryStart();

            try
            {
                _readResult = await StartTimingReadAsync(cancellationToken);
            }
            catch (ConnectionAbortedException ex)
            {
                throw new TaskCanceledException("The request was aborted", ex);
            }

            StopTimingRead(_readResult.Buffer.Length);

            if (_readResult.IsCompleted)
            {
                TryStop();
            }

            return _readResult;
        }

        private ValueTask<ReadResult> StartTimingReadAsync(CancellationToken cancellationToken)
        {
            var readAwaitable = _context.RequestPipe.Reader.ReadAsync(cancellationToken);

            if (!readAwaitable.IsCompleted && _timingEnabled)
            {
                _backpressure = true;
                _context.TimeoutControl.StartTimingRead();
            }

            return readAwaitable;
        }

        private void StopTimingRead(long bytesRead)
        {
            _context.TimeoutControl.BytesRead(bytesRead - _alreadyTimedBytes);
            _alreadyTimedBytes = 0;

            if (_backpressure)
            {
                _backpressure = false;
                _context.TimeoutControl.StopTimingRead();
            }
        }

        public override void Complete(Exception exception)
        {
            _context.RequestPipe.Reader.Complete(exception);
        }

        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            _context.RequestPipe.Reader.OnWriterCompleted(callback, state);
        }

        public override void CancelPendingRead()
        {
            _context.RequestPipe.Reader.CancelPendingRead();
        }

        protected override Task OnStopAsync()
        {
            if (!_context.HasStartedConsumingRequestBody)
            {
                return Task.CompletedTask;
            }

            _context.RequestPipe.Reader.Complete();

            return Task.CompletedTask;
        }
    }
}
