using UnityEngine;

namespace Assets.Scripts.Main
{
    public class Train : TrainComponent
    {
        private readonly MovingPoint _pointFront = new MovingPoint();

        protected override void Start()
        {
            base.Start();
            _pointFront.CurrentRoad = _pointBack.CurrentRoad;
            if (_pointFront?.CurrentRoad != null)
            {
                _pointFront.FromIndex = _pointFront.CurrentRoad.Value.index1;
            }
            _pointFront.Time += Others;
        }

        protected override void InitData()
        {
            UpdatePositions(_pointFront);

            base.InitData();
        }

        protected override void SetPosition()
        {
            if (_pointBack.CurrentRoad == null) return;
            var pos = (_pointBack.Position + _pointFront.Position) * 0.5f;

            Debug.DrawLine(_pointBack.Position, _pointBack.Position + Vector3.up, Color.red);
            Debug.DrawLine(_pointFront.Position, _pointFront.Position + Vector3.up, Color.red);

            transform.position = pos;
            transform.rotation = Quaternion.LookRotation(_pointFront.Position - _pointBack.Position, Vector3.up);
        }

        private void Update()
        {
            UpdatePositions(_pointBack);
            UpdatePositions(_pointFront);
            SetPosition();
        }
    }
}
