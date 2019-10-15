namespace DOTS
{
    internal struct Maybe<T>
    {
        public static Maybe<T> Nothing { get; set; }
        public bool HasValue { get; set; }
        public AStarNode Value { get; set; }

        public Maybe(AStarNode node)
        {
            throw new System.NotImplementedException();
        }
    }
}