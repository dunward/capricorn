#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace Dunward.Capricorn
{
    public class ConnectorNode : BaseNode
    {
        private TextField customField;

        public ConnectorNode() : base()
        {
            nodeType = NodeType.Connector;
        }

        public ConnectorNode(GraphView graphView, int id, float x, float y) : base(graphView, id, x, y)
        {
        }

        public ConnectorNode(GraphView graphView, int id, Vector2 mousePosition) : base(graphView, id, mousePosition)
        {
        }

        public ConnectorNode(GraphView graphView, NodeMainData mainData) : base(graphView, mainData)
        {
        }

        private void UpdateSubContainers(NodeMainData mainData)
        {
            main.action.DeserializeConnections(mainData.actionData);
        }

        protected override void SetupTitleContainer()
        {
            customField = new TextField() { value = string.IsNullOrEmpty(customTitle) ? $"{id}" : customTitle };
            customField.RegisterValueChangedCallback(evt =>
            {
                customTitle = evt.newValue;
            });
            customField.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (string.IsNullOrEmpty(customField.value))
                {
                    customTitle = string.Empty;
                    customField.value = $"{id}";
                }
            });

            titleContainer.Insert(0, customField);
        }

        protected override void Repaint()
        {
            customField.value = string.IsNullOrEmpty(customTitle) ? $"{id}" : customTitle;
        }
    }
}
#endif