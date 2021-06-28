using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class RailRode : MonoBehaviour
    {
        public Crossing[] Crossings = {};
        
        public List<Road> roads = new List<Road>();

        private CatmullMesh catmullMesh;

        void Start()
        {
            CreateMesh();
        }

        public Road GetRoad()
        {
            return roads.FirstOrDefault();
        }

        public Road? NextRoad(Road prevRoad, int index)
        {
            foreach (var possibleNextRoad in roads.Where(road => road.index2 == index || road.index1 == index))
            {
                if (!prevRoad.Equals(possibleNextRoad))
                    return possibleNextRoad;
            }

            return null;
        }

        public bool IsClose(Road road, Vector3 position, int from)
        {
            var crossing = Crossings[road.Other(from)];

            return (crossing.transform.position - position).magnitude < 0.4f;
        }

        public Vector3 GetPosition(Road road, float time, int revIndex)
        {
            var c1 = Crossings[revIndex];
            var c2 = Crossings[road.Other(revIndex)];

            var dir = (c1.transform.position - c2.transform.position).normalized;
            var bestVector1 = BestVector(c1.transform, -dir);
            var bestVector2 = BestVector(c2.transform, dir);

            var p0 = Crossings[revIndex].transform.position;
            var p1 = Crossings[revIndex].transform.position + bestVector1;


            var p2 = Crossings[road.Other(revIndex)].transform.position + bestVector2;
            var p3 = Crossings[road.Other(revIndex)].transform.position;

            return catmullMesh.GetPosition(p0, p1, p2, p3, time);
        }

        public void CreateMesh()
        {
            catmullMesh = new CatmullMesh();
            GetComponent<MeshFilter>().mesh = catmullMesh._mesh;
            catmullMesh.CleareShape();

            foreach (var road in roads)
            {
                var c1 = Crossings[road.index1];
                var c2= Crossings[road.index2];
                
                var dir = (c1.transform.position - c2.transform.position).normalized;
                var bestVector1 = BestVector(c1.transform, -dir);
                var bestVector2 = BestVector(c2.transform, dir);
                
                var p0 = Crossings[road.index1].transform.position;
                var p1 = Crossings[road.index1].transform.position + bestVector1;


                var p2 = Crossings[road.index2].transform.position + bestVector2;
                var p3 = Crossings[road.index2].transform.position;

                catmullMesh.AddCatmullLine(p0, p1, p2, p3);
            }

            catmullMesh.UpdateMesh();
        }

        private Vector3 BestVector(Transform crossing, Vector3 dir)
        {
            var options = new[]
            {
                crossing.forward.normalized,
                crossing.right.normalized,
                -crossing.forward.normalized,
                -crossing.right.normalized,
            };
            var bestVector = options[0];
            foreach (var currentVector in options)
            {
                if (Vector3.Dot(currentVector, dir) > Vector3.Dot(bestVector, dir))
                    bestVector = currentVector;
            }

            return bestVector;
        }
    }

    [Serializable]
    public struct Road : IEquatable<Road>
    {
        public int index1;
        public int index2;

        public int Other(int same)
        {
            return same == index1 ? index2 : index1;
        }

        public override bool Equals(object obj)
        {
            return obj is Road other && Equals(other);
        }

        public bool Equals(Road other)
        {
            return index1 == other.index1 && index2 == other.index2 || index1 == other.index2 && index2 == other.index1;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return index1 < index2 ? (index1 * 397) ^ index2 : (index2 * 397) ^ index1;
            }
        }
    }
}
