namespace AI_ClothColliders
{
    internal sealed class SphereColliderPair
    {
        public SphereColliderData first;
        public SphereColliderData second;

        public SphereColliderPair(SphereColliderData first, SphereColliderData second)
        {
            this.first = first;
            this.second = second;
        }
    }
}