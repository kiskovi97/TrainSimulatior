using System.Linq;
using Assets.Scripts.Main;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof(RailRode))]
    class RailRodeEditor : UnityEditor.Editor
    {

        private RailRode targetComponent;

        private int index;
        private int index2;

        private void OnSceneGUI()
        {
            targetComponent = (RailRode)target;
            var crossings = targetComponent.Crossings;

            Handles.color = Color.blue;

            foreach (var road in targetComponent.roads.Where(road => crossings.Length > road.index2 && crossings.Length > road.index1))
            {
                Handles.DrawLine(crossings[road.index1].transform.position, crossings[road.index2].transform.position);
            }

            Handles.color = Color.green;


            var crossing = crossings[index];
            if (crossing != null)
            {
                var crossing2 = crossings[index2];

                if (crossing2 != crossing)
                    Handles.DrawLine(crossing.transform.position, crossing2.transform.position);

            }

            foreach (var crossingCurrent in crossings)
            {
                crossingCurrent.transform.rotation = Handles.RotationHandle(crossingCurrent.transform.rotation, crossingCurrent.transform.position);
                crossingCurrent.transform.position = Handles.PositionHandle(crossingCurrent.transform.position, Quaternion.identity);
            }

            targetComponent.CreateMesh();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            targetComponent = (RailRode)target;

            if (GUILayout.Button("Next Index"))
            {
                index++;
                if (index2 == index) index++;
                if (index >= targetComponent.Crossings.Length)
                {
                    index = index2 == 0 ? 1 : 0;
                }
            }

            if (GUILayout.Button("Next Index 2"))
            {
                index2++;
                if (index2 == index) index2++;
                if (index2 >= targetComponent.Crossings.Length)
                {
                    index2 = index == 0 ? 1 : 0;
                }
            }

            if (GUILayout.Button("Add Indexes"))
            {
                var road = new Road() {index1 = index, index2 = index2};
                var road2 = new Road() { index1 = index2, index2 = index };
                if (!targetComponent.roads.Contains(road) && !targetComponent.roads.Contains(road2))
                    targetComponent.roads.Add(road);
            }

            SceneView.RepaintAll();
        }
    }
}
