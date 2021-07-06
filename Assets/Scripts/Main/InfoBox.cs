using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Assets.Scripts.Main
{
    [RequireComponent(typeof(CrossingUI))]
    class InfoBox : Button
    {
        public MoveCamera target;
        private RectTransform rectTransform;
        private CrossingUI ui;

        private void Start()
        {
            ui = GetComponent<CrossingUI>();
            rectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (target.SelectedCrossing == null) return;
            var pos = Camera.main.WorldToScreenPoint(target.SelectedCrossing.transform.position);
            rectTransform.position = pos;
            target.SelectedCrossing.Draw();
            var positions = target.SelectedCrossing.GetPositions();

            var pos1 = Camera.main.WorldToScreenPoint(positions[0]);
            var pos2 = Camera.main.WorldToScreenPoint(positions[1]);
            var dir1 = rectTransform.position - pos1;
            var dir2 = rectTransform.position - pos2;
            var radius = rectTransform.rect.x + 10f;
            ui.image1.position = rectTransform.position + dir1.normalized * radius;
            ui.image2.position = rectTransform.position + dir2.normalized * radius;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            target.SelectedCrossing.Switch();
        }
    }
}
