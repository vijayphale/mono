//
// System.Drawing.Imaging.EncoderParameter.cs
//
// Author: 
//	Ravindra (rkumar@novell.com)
//  Vladimir Vukicevic (vladimir@pobox.com)
//
// (C) 2004 Novell, Inc.  http://www.novell.com
//

using System;
using System.Text;

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging {

	public sealed class EncoderParameter : IDisposable {

		private Encoder encoder;
		private int valuesCount;
		private EncoderParameterValueType type;
		private IntPtr valuePtr;

		internal EncoderParameter ()
		{
		}

		public EncoderParameter (Encoder encoder, byte value)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeByte;
			this.valuePtr = Marshal.AllocHGlobal (1);
			Marshal.WriteByte (this.valuePtr, value);
		}

		public EncoderParameter (Encoder encoder, byte[] value)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			this.type = EncoderParameterValueType.ValueTypeByte;
			this.valuePtr = Marshal.AllocHGlobal (1 * valuesCount);
			Marshal.Copy (value, 0, this.valuePtr, valuesCount);
		}

		public EncoderParameter (Encoder encoder, short value)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeShort;
			this.valuePtr = Marshal.AllocHGlobal (2);
			Marshal.WriteInt16 (this.valuePtr, value);
		}

		public EncoderParameter (Encoder encoder, short[] value)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			this.type = EncoderParameterValueType.ValueTypeShort;
			this.valuePtr = Marshal.AllocHGlobal (2 * valuesCount);
			Marshal.Copy (value, 0, this.valuePtr, valuesCount);
		}


		public EncoderParameter (Encoder encoder, long value)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeLong;
			this.valuePtr = Marshal.AllocHGlobal (4);
			Marshal.WriteInt32 (this.valuePtr, (int) value);
		}

		public EncoderParameter (Encoder encoder, long[] value)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			this.type = EncoderParameterValueType.ValueTypeLong;
			this.valuePtr = Marshal.AllocHGlobal (4 * valuesCount);
			int [] ivals = new int[value.Length];
			for (int i = 0; i < value.Length; i++) ivals[i] = (int) value[i];
			Marshal.Copy (ivals, 0, this.valuePtr, valuesCount);
		}

		public EncoderParameter (Encoder encoder, string value)
		{
			this.encoder = encoder;

			ASCIIEncoding ascii = new ASCIIEncoding ();
			int asciiByteCount = ascii.GetByteCount (value);
			byte[] bytes = new byte [asciiByteCount];
			ascii.GetBytes (value, 0, value.Length, bytes, 0);

			this.valuesCount = bytes.Length;
			this.type = EncoderParameterValueType.ValueTypeAscii;
			this.valuePtr = Marshal.AllocHGlobal (valuesCount);
			Marshal.Copy (bytes, 0, this.valuePtr, valuesCount);
		}

		public EncoderParameter (Encoder encoder, byte value, bool undefined)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			if (undefined)
				this.type = EncoderParameterValueType.ValueTypeUndefined;
			else
				this.type = EncoderParameterValueType.ValueTypeByte;
			this.valuePtr = Marshal.AllocHGlobal (1);
			Marshal.WriteByte (this.valuePtr, value);
		}

		public EncoderParameter (Encoder encoder, byte[] value, bool undefined)
		{
			this.encoder = encoder;
			this.valuesCount = value.Length;
			if (undefined)
				this.type = EncoderParameterValueType.ValueTypeUndefined;
			else
				this.type = EncoderParameterValueType.ValueTypeByte;
			this.valuePtr = Marshal.AllocHGlobal (valuesCount);
			Marshal.Copy (value, 0, this.valuePtr, valuesCount);
		}

		public EncoderParameter (Encoder encoder, int numerator, int denominator)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeRational;
			this.valuePtr = Marshal.AllocHGlobal (8);
			int [] valuearray = { numerator, denominator };
			Marshal.Copy (valuearray, 0, this.valuePtr, valuearray.Length);
		}

		public EncoderParameter (Encoder encoder, int[] numerator, int[] denominator)
		{
			if (numerator.Length != denominator.Length)
				throw new ArgumentException ("Invalid parameter used.");

			this.encoder = encoder;
			this.valuesCount = numerator.Length;
			this.type = EncoderParameterValueType.ValueTypeRational;
			this.valuePtr = Marshal.AllocHGlobal (4 * valuesCount * 2);
			IntPtr dest = this.valuePtr;
			for (int i = 0; i < valuesCount; i++) {
				Marshal.WriteInt32 (dest, (int) numerator[i]);
				dest = (IntPtr) ((int) dest + 4);
				Marshal.WriteInt32 (dest, (int) denominator[i]);
				dest = (IntPtr) ((int) dest + 4);
			}
		}

		public EncoderParameter (Encoder encoder, long rangebegin, long rangeend)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeLongRange;
			this.valuePtr = Marshal.AllocHGlobal (8);
			int [] valuearray = { (int) rangebegin, (int) rangeend };
			Marshal.Copy (valuearray, 0, this.valuePtr, valuearray.Length);
		}

		public EncoderParameter (Encoder encoder, long[] rangebegin, long[] rangeend)
		{
			if (rangebegin.Length != rangeend.Length)
				throw new ArgumentException ("Invalid parameter used.");

			this.encoder = encoder;
			this.valuesCount = rangebegin.Length;
			this.type = EncoderParameterValueType.ValueTypeLongRange;

			this.valuePtr = Marshal.AllocHGlobal (4 * valuesCount * 2);
			IntPtr dest = this.valuePtr;
			for (int i = 0; i < valuesCount; i++) {
				Marshal.WriteInt32 (dest, (int) rangebegin[i]);
				dest = (IntPtr) ((int) dest + 4);
				Marshal.WriteInt32 (dest, (int) rangeend[i]);
				dest = (IntPtr) ((int) dest + 4);
			}
		}

		public EncoderParameter (Encoder encoder, int numberOfValues, int type, int value)
		{
			this.encoder = encoder;
			this.valuePtr = (IntPtr) value;
			this.valuesCount = numberOfValues;
			this.type = (EncoderParameterValueType) type;
		}

		public EncoderParameter (Encoder encoder, int numerator1, int denominator1, int numerator2, int denominator2)
		{
			this.encoder = encoder;
			this.valuesCount = 1;
			this.type = EncoderParameterValueType.ValueTypeRationalRange;
			this.valuePtr = Marshal.AllocHGlobal (4 * 4);
			int [] valuearray = { numerator1, denominator1, numerator2, denominator2 };
			Marshal.Copy (valuearray, 0, this.valuePtr, 4);
		}

		public EncoderParameter (Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
		{
			if (numerator1.Length != denominator1.Length ||
			    numerator2.Length != denominator2.Length ||
			    numerator1.Length != numerator2.Length)
				throw new ArgumentException ("Invalid parameter used.");

			this.encoder = encoder;
			this.valuesCount = numerator1.Length;
			this.type = EncoderParameterValueType.ValueTypeRationalRange;

			this.valuePtr = Marshal.AllocHGlobal (4 * valuesCount * 4);
			IntPtr dest = this.valuePtr;
			for (int i = 0; i < valuesCount; i++) {
				Marshal.WriteInt32 (dest, numerator1[i]);
				dest = (IntPtr) ((int) dest + 4);
				Marshal.WriteInt32 (dest, denominator1[i]);
				dest = (IntPtr) ((int) dest + 4);
				Marshal.WriteInt32 (dest, numerator2[i]);
				dest = (IntPtr) ((int) dest + 4);
				Marshal.WriteInt32 (dest, denominator2[i]);
				dest = (IntPtr) ((int) dest + 4);
			}
		}

		public Encoder Encoder {
			get {
				return encoder;
			}

			set {
				encoder = value;
			}
		}

		public int NumberOfValues {
			get {
				return valuesCount;
			}
		}

		public EncoderParameterValueType Type {
			get {
				return type;
			}
		}

		public EncoderParameterValueType ValueType {
			get {
				return type;
			}
		}

		void Dispose (bool disposing) {
			if (valuePtr != IntPtr.Zero) {
				Marshal.FreeHGlobal (valuePtr);
				valuePtr = IntPtr.Zero;
			}
		}

		public void Dispose () {
			Dispose (true);		
		}

		~EncoderParameter () {
			Dispose (false);
		}

		internal static int NativeSize () {
			return Marshal.SizeOf (typeof(GdipEncoderParameter));
		}

		internal void ToNativePtr (IntPtr epPtr) {
			GdipEncoderParameter ep = new GdipEncoderParameter ();
			ep.guid = this.encoder.Guid;
			ep.numberOfValues = (uint) this.valuesCount;
			ep.type = this.type;
			ep.value = this.valuePtr;
			Marshal.StructureToPtr (ep, epPtr, false);
		}

		internal static EncoderParameter FromNativePtr (IntPtr epPtr) {
			GdipEncoderParameter ep;
			ep = (GdipEncoderParameter) Marshal.PtrToStructure (epPtr, typeof(GdipEncoderParameter));

			Type valType;
			uint valCount;

			switch (ep.type) {
			case EncoderParameterValueType.ValueTypeAscii:
			case EncoderParameterValueType.ValueTypeByte:
			case EncoderParameterValueType.ValueTypeUndefined:
				valType = typeof(byte);
				valCount = ep.numberOfValues;
				break;
			case EncoderParameterValueType.ValueTypeShort:
				valType = typeof(short);
				valCount = ep.numberOfValues;
				break;
			case EncoderParameterValueType.ValueTypeLong:
				valType = typeof(int);
				valCount = ep.numberOfValues;
				break;
			case EncoderParameterValueType.ValueTypeLongRange:
			case EncoderParameterValueType.ValueTypeRational:
				valType = typeof(int);
				valCount = ep.numberOfValues * 2;
				break;
			case EncoderParameterValueType.ValueTypeRationalRange:
				valType = typeof(int);
				valCount = ep.numberOfValues * 4;
				break;
			default:
				return null;
			}

			EncoderParameter eparam = new EncoderParameter();
			eparam.encoder = new Encoder(ep.guid);
			eparam.valuesCount = (int) ep.numberOfValues;
			eparam.type = ep.type;
			eparam.valuePtr = Marshal.AllocHGlobal ((int)(valCount * Marshal.SizeOf(valType)));

			/* There's nothing in Marshal to do a memcpy() between two IntPtrs.  This sucks. */
			unsafe {
				byte *s = (byte *) ep.value;
				byte *d = (byte *) eparam.valuePtr;
				for (int i = 0; i < valCount * Marshal.SizeOf(valType); i++)
					*d++ = *s++;
			}

			return eparam;
		}
	}
}
