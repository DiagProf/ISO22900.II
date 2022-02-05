namespace ISO22900.II
{
    public class PduFlagDataCllCreateFlag
    {
        public byte[] FlagData { get; } = new byte[4] {0x40, 0x00, 0x00, 0x00};

        public void RawMode(bool flag)
        {
            if (flag)
                FlagData[0] |= 0x80;
            else
                FlagData[0] &= 0x7F;
        }

        public void ChecksumMode(bool flag)
        {
            if (flag)
                FlagData[0] |= 0x40;
            else
                FlagData[0] &= 0xBF;
        }

        public void TxMsgDoneTimestampIndicator(bool flag)
        {
            if (flag)
                FlagData[0] |= 0x80;
            else
                FlagData[0] &= 0x7F;
        }

        public void StartMsgTimestampIndicator(bool flag)
        {
            if (flag)
                FlagData[0] |= 0x40;
            else
                FlagData[0] &= 0xBF;
        }
    }
}