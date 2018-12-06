using System.IO;
using LibProShip.Domain.Decode;

namespace LibProShip.Domain
{
    public interface IDecoder
    {
        /// <summary>
        /// Decode a file to raw replay
        /// </summary>
        /// <param name="replayFile"></param>
        /// <returns>
        ///
        /// Raw replay
        /// 
        /// null if unable to decode
        /// </returns>
        RawReplay DecodeReplay(FileInfo replayFile);
    }
}