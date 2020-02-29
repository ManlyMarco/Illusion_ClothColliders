namespace AI_ClothColliders
{
    internal sealed class SphereColliderPair
    {
        public string ClothName { get; }
        public SphereColliderData first;
        public SphereColliderData second;

        public SphereColliderPair(SphereColliderData first, SphereColliderData second, string clothName)
        {
            ClothName = clothName;
            this.first = first;
            this.second = second;
        }
    }
}