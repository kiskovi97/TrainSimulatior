using System;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class TrainComponent : MonoBehaviour
    {
        public RailRode RailRode;
        public TrainComponent ForwardTrainComponent;

        protected readonly MovingPoint _pointBack = new MovingPoint();
        public float Speed = 5f;
        protected const float Others = 3.5f;

        public class MovingPoint
        {
            public Road? CurrentRoad;

            public float Time = 0f;

            public int FromIndex = 0;

            public Vector3 Position;
        }

        public event Action Updated;

        protected virtual void Start()
        {
            _pointBack.CurrentRoad = RailRode.GetRoad();
            if (_pointBack?.CurrentRoad != null)
            {
                _pointBack.FromIndex = _pointBack.CurrentRoad.Value.index1;
            }

            _pointBack.Position = transform.position;

            ForwardTrainComponent.Updated += () =>
            {
                if (ForwardTrainComponent != null)
                {
                    _pointBack.Time = ForwardTrainComponent._pointBack.Time - Others;
                }

                UpdatePositions(_pointBack);
                Updated?.Invoke();
            };

            UpdatePositions(_pointBack);
            InvokeUpdate();
        }

        protected void InvokeUpdate()
        {
            Updated?.Invoke();
        }

        private void Update()
        {
            UpdatePositions(_pointBack);
            SetPosition();
        }

        protected virtual void SetPosition()
        {
            if (_pointBack.CurrentRoad == null) return;
            var pos = (_pointBack.Position + ForwardTrainComponent._pointBack.Position) * 0.5f;
            Debug.DrawLine(_pointBack.Position, _pointBack.Position + Vector3.up * 3f, Color.red);

            //transform.position += (pos - transform.position).normalized * Speed * Time.deltaTime;
            transform.position = pos;
            transform.rotation = Quaternion.LookRotation(ForwardTrainComponent._pointBack.Position - _pointBack.Position, Vector3.up);
        }

        protected void UpdatePositions(MovingPoint movingPoint)
        {
            movingPoint.Time += Time.deltaTime * Speed;
            if (movingPoint.CurrentRoad == null) return;
            movingPoint.Position = RailRode.GetPosition(movingPoint.CurrentRoad.Value, movingPoint.Time, movingPoint.FromIndex);

            if (!RailRode.IsClose(movingPoint.CurrentRoad.Value, movingPoint.Time, movingPoint.FromIndex)) return;

            movingPoint.FromIndex = movingPoint.CurrentRoad.Value.Other(movingPoint.FromIndex);
            movingPoint.CurrentRoad = RailRode.NextRoad(movingPoint.CurrentRoad.Value, movingPoint.FromIndex);
            movingPoint.Time = 0f;

        }
    }
}
