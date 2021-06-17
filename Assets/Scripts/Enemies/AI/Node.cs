using UnityEngine;

namespace Enemies.AI
{
    public class Node
    {
        public bool Walkable;
        public Vector3 WorldPosition;

        public int XGridPos;
        public int YGridPos;


        public int gCost;
        public int hCost;

        public int fCost => gCost + hCost;

        public Node parent;


        public Node(bool walkable, Vector3 worldPosition, int xGridPos, int yGridPos)
        {
            Walkable = walkable;
            WorldPosition = worldPosition;

            XGridPos = xGridPos;
            YGridPos = yGridPos;

            gCost = 0;
            hCost = 0;
        }

        public override bool Equals(object obj)
        {
            return obj != null && WorldPosition == ((Node) obj).WorldPosition;
        }

        public static bool operator ==(Node a, Node b)
        {
            return a.WorldPosition == b.WorldPosition;
        }

        public static bool operator !=(Node a, Node b)
        {
            return !(a == b);
        }
    }
}