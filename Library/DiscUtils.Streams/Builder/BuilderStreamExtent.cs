//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using DiscUtils.Streams.Compatibility;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscUtils.Streams;

public class BuilderStreamExtent : BuilderExtent
{
    private readonly Ownership _ownership;
    private Stream _source;

    public BuilderStreamExtent(long start, Stream source)
        : this(start, source, Ownership.None) {}

    public BuilderStreamExtent(long start, Stream source, Ownership ownership)
        : base(start, source.Length)
    {
        _source = source;
        _ownership = ownership;
    }

    protected override void Dispose(bool disposing)
    {
        if (_source != null && _ownership == Ownership.Dispose)
        {
            if (disposing)
            {
                _source.Dispose();
            }

            _source = null;
        }
    }

    public override void PrepareForRead() {}

    public override int Read(long diskOffset, byte[] block, int offset, int count)
    {
        _source.Position = diskOffset - Start;
        return _source.Read(block, offset, count);
    }

    public override Task<int> ReadAsync(long diskOffset, byte[] block, int offset, int count, CancellationToken cancellationToken)
    {
        _source.Position = diskOffset - Start;
        return _source.ReadAsync(block, offset, count, cancellationToken);
    }

    public override ValueTask<int> ReadAsync(long diskOffset, Memory<byte> block, CancellationToken cancellationToken)
    {
        _source.Position = diskOffset - Start;
        return _source.ReadAsync(block, cancellationToken);
    }

    public override int Read(long diskOffset, Span<byte> block)
    {
        _source.Position = diskOffset - Start;
        return _source.Read(block);
    }

    public override void DisposeReadState() {}
}