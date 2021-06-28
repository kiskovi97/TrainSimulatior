using UnityEngine;

namespace Assets.Scripts.Main
{
    public class Train : TrainComponent
    {
        private readonly MovingPoint _pointFront = new MovingPoint();

        protected override void Start()
        {
            _pointBack.CurrentRoad = RailRode.GetRoad();
            _pointFront.CurrentRoad = _pointBack.CurrentRoad;
            if (_pointBack?.CurrentRoad != null)
            {
                _pointBack.FromIndex = _pointBack.CurrentRoad.Value.index1;
            }
            if (_pointFront?.CurrentRoad != null)
            {
                _pointFront.FromIndex = _pointFront.CurrentRoad.Value.index1;
            }

            _pointBack.Position = transform.position;
            _pointFront.Position = transform.position;
            _pointFront.Time += Others;

            UpdatePositions(_pointBack);
            UpdatePositions(_pointFront);

            transform.position = _pointFront.Position;

            InvokeUpdate();
        }

        protected override void SetPosition()
        {
            if (_pointBack.CurrentRoad == null) return;
            var pos = (_pointBack.Position + _pointFront.Position) * 0.5f;

            Debug.DrawLine(_pointBack.Position, _pointBack.Position + Vector3.up, Color.red);
            Debug.DrawLine(_pointFront.Position, _pointFront.Position + Vector3.up, Color.red);
            //transform.position += (pos - transform.position).normalized * Speed * 2f * Time.deltaTime;
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
