using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace LedMusic2.VstInterop
{

    public class VstChannel : IDisposable
    {

        public Guid Id { get; }
        public VstChannelType Type { get; }
        public string Name { get; private set; }
        public double Value { get; private set; }
        public Note[] Notes { get; private set; } = new Note[16];

        private readonly MemoryMappedFile mf;
        private readonly MemoryMappedViewAccessor va;

        public VstChannel(Guid id, VstChannelType type)
        {
            Id = id;
            Type = type;
            Name = id.ToString();
            mf = MemoryMappedFile.CreateNew("LedMusicVST_" + id.ToString(), calcFileSize());
            va = mf.CreateViewAccessor();
        }

        public void UpdateValues()
        {
            switch (Type)
            {
                case VstChannelType.MIDI:
                    parseMidi();
                    break;
                case VstChannelType.VALUE:
                    Value = va.ReadDouble(0);
                    break;
            }
        }

        private void parseMidi()
        {
            var data = new byte[3];
            va.ReadArray(0, data, 0, 3);
            va.WriteArray(0, new byte[3] { 0, 0, 0 }, 0, 3);
            var channel = data[0] & 0x0F;
            if ((data[0] & 0xF0) == 0x80)
            {
                // Note off
                Debug.WriteLine($"Note off (Channel {channel})");
                if (Notes[channel].Number == data[1])
                {
                    Notes[channel].Number = 0;
                    Notes[channel].Velocity = 0;
                }
            } else if ((data[0] & 0xF0) == 0x90)
            {
                // Note on event
                Debug.WriteLine($"Note on (Channel {channel}, Note {data[1]}, Velocity {data[2]}");
                Notes[channel].Number = data[1];
                Notes[channel].Velocity = data[2];
            }
        }

        private int calcFileSize()
        {
            switch (Type)
            {
                case VstChannelType.MIDI:
                    return 3;
                case VstChannelType.VALUE:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException("type", "Invalid channel type");                
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    va.Dispose();
                    mf.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }

}
