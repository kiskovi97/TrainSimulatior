using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class Crossing : MonoBehaviour
    {
        private readonly List<Road> roads = new List<Road>();
        private readonly List<Vector3> goals = new List<Vector3>();

        public void Add(Road road, Vector3 goal)
        {
            roads.Add(road);
            goals.Add(goal);
        }

        public Road NextRoad(Road prevRoad)
        {
            return roads.FirstOrDefault(road => !road.Equals(prevRoad));
        }

        public void Switch()
        {
            var road = roads[0];
            roads.RemoveAt(0);
            roads.Add(road);
            
            var goal = goals[0];
            goals.RemoveAt(0);
            goals.Add(goal);
        }

        public void Draw()
        {
            Debug.DrawLine(goals[0], transform.position, Color.green);
            Debug.DrawLine(goals[1], transform.position, Color.red);
        }

        public Vector3[] GetPositions()
        {
            return goals.ToArray();
        }
    }
}
