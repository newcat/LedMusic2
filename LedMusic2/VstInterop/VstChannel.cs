using LedMusic2.Reactive;
using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace LedMusic2.VstInterop
{

    public class VstChannel : ReactiveObject, IReactiveListItem, ICombinedReactive, IDisposable
    {

        private const int MIDI_PACKET_COUNT = 5;
        
        public Guid Id { get; set; }
        public ReactivePrimitive<VstChannelType> Type { get; } = new ReactivePrimitive<VstChannelType>();
        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>();
        public double Value { get; private set; }
        public Note[] Notes { get; private set; } = new Note[16];

        private MemoryMappedFile mf;
        private MemoryMappedViewAccessor va;

        public VstChannel() { }

        public VstChannel(Guid id, VstChannelType type)
        {
            Id = id;
            Type.Set(type);
            Name.Set(id.ToString());
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
            mf = MemoryMappedFile.CreateNew("LedMusicVST_" + Id.ToString(), calcFileSize());
            va = mf.CreateViewAccessor();
        }

        public void UpdateValues()
        {
            switch (Type.Get())
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
            var data = new byte[3 * MIDI_PACKET_COUNT];
            va.ReadArray(0, data, 0, 3 * MIDI_PACKET_COUNT);
            va.WriteArray(0, new byte[15], 0, 3 * MIDI_PACKET_COUNT);
            for (var i = 0; i < MIDI_PACKET_COUNT; i++)
            {
                var channel = data[3 * i] & 0x0F;
                if ((data[3 * i] & 0xF0) == 0x80)
                {
                    // Note off
                    Debug.WriteLine($"Note off (Channel {channel})");
                    if (Notes[channel].Number == data[3 * i + 1])
                    {
                        Notes[channel].Number = 0;
                        Notes[channel].Velocity = 0;
                    }
                }
                else if ((data[3 * i] & 0xF0) == 0x90)
                {
                    // Note on event
                    Debug.WriteLine($"Note on (Channel {channel}, Note {data[3 * i + 1]}, Velocity {data[3 * i + 2]}");
                    Notes[channel].Number = data[3 * i + 1];
                    Notes[channel].Velocity = data[3 * i + 2];
                }
            }
        }

        private int calcFileSize()
        {
            switch (Type.Get())
            {
                case VstChannelType.MIDI:
                    return 3 * MIDI_PACKET_COUNT;
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
