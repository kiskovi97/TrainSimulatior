using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Main
{
    public class RailRode : MonoBehaviour
    {
        public Crossing[] Crossings = {};
        
        [SerializeField]
        private List<Road> roads = new List<Road>();

        public IEnumerable<Road> Roads => roads;

        private CatmullMesh catmullMesh;

        public void AddRoad(int index, int index2)
        {
            if (index >= roads.Count || index2 >= roads.Count) return;

            var road = new Road() { index1 = index, index2 = index2 };
            var road2 = new Road() { index1 = index2, index2 = index };
            if (roads.Contains(road) || roads.Contains(road2)) return;

            roads.Add(road);
        }

        private void Start()
        {
            CreateMesh();
            foreach (var road in roads)
            {
                var cross = Crossings[road.index1];
                var cross2 = Crossings[road.index2];

                cross.Add(road, cross2.transform.position);
                cross2.Add(road, cross.transform.position);
            }
        }

        public Road GetRoad(Vector3 position)
        {
            var selected = 0;
            for (var i = 1; i < Crossings.Length; i++)
            {
                if ((Crossings[selected].transform.position - position).sqrMagnitude
                    > (Crossings[i].transform.position - position).sqrMagnitude)
                    selected = i;
            }
            return roads.FirstOrDefault(data => data.index1 == selected || data.index2 == selected);
        }

        public Crossing GetCrossing(Vector3 position)
        {
            var selected = 0;
            for (var i = 1; i < Crossings.Length; i++)
            {
                if ((Crossings[selected].transform.position - position).sqrMagnitude
                    > (Crossings[i].transform.position - position).sqrMagnitude)
                    selected = i;
            }

            return Crossings[selected];
        }

        public Road? NextRoad(Road prevRoad, int index)
        {
            return Crossings[index].NextRoad(prevRoad);
        }

        public bool IsClose(Road road, float time, int revIndex)
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

            return catmullMesh != null && catmullMesh.IsOver(p0, p1, p2, p3, time);
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

            if (catmullMesh != null) return catmullMesh.GetPosition(p0, p1, p2, p3, time);
            return Vector3.zero;
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

        private static Vector3 BestVector(Transform crossing, Vector3 dir)
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

            return bestVector * 1.5f;
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
