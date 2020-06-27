namespace Toms_Puzzle.Interfaces
{
    public interface ILayer
    {
        public string Decode(string payload, IDecoder decoder);
    }
}
