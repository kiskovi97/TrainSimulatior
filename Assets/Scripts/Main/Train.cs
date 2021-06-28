using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Main
{
    class Train: MonoBehaviour
    {
        public Road? currentRoad;

        public RailRode railRode;

        private float _time = 0f;

        public float speed = 5f;

        private int fromIndex = 0;

        private void Start()
        {
            currentRoad = railRode.GetRoad();
            if (currentRoad != null)
            {
                fromIndex = currentRoad.Value.index1;
                transform.position = railRode.GetPosition(currentRoad.Value, 0f, fromIndex);
            }
        }

        private void Update()
        {
            _time += Time.deltaTime * speed;
            if (currentRoad != null)
            {
                transform.position = railRode.GetPosition(currentRoad.Value, _time, fromIndex);
                if (railRode.IsClose(currentRoad.Value, transform.position, fromIndex))
                {
                    fromIndex = currentRoad.Value.Other(fromIndex);
                    currentRoad = railRode.NextRoad(currentRoad.Value, fromIndex);
                    Debug.Log("Next Road");
                    _time = 0f;
                }
            }
        }
    }
}
