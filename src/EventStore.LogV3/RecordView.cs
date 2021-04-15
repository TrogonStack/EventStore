﻿using System;
using System.Runtime.InteropServices;

namespace EventStore.LogV3 {
	// Immutable view of a record
	public struct RecordView<TSubHeader> where TSubHeader : unmanaged {
		private readonly ReadOnlySlicedRecord _sliced;

		public ReadOnlyMemory<byte> Bytes => _sliced.Bytes;
		public ref readonly Raw.RecordHeader Header => ref MemoryMarshal.AsRef<Raw.RecordHeader>(_sliced.HeaderMemory.Span);
		public ref readonly TSubHeader SubHeader => ref MemoryMarshal.AsRef<TSubHeader>(_sliced.SubHeaderMemory.Span);
		public ReadOnlyMemory<byte> Payload => _sliced.PayloadMemory;
		public int PayloadOffset => _sliced.HeaderMemory.Length + _sliced.SubHeaderMemory.Length;

		public RecordView(ReadOnlyMemory<byte> bytes) {
			_sliced = SlicedRecordCreator<TSubHeader>.Create(bytes);
		}
	}
}
