using System.Collections;
using System.Collections.Generic;

namespace Dunward.Capricorn
{
    public partial class CapricornRunner
    {
        internal GraphData graphData;
        
        internal UnityEngine.MonoBehaviour target;
        internal Dictionary<int, NodeMainData> nodes = new Dictionary<int, NodeMainData>();
        
        public System.Action onInteraction;
        public event CoroutineDelegate AddCustomCoroutines;
        public delegate IEnumerator CoroutineDelegate(CoroutineUnit unit);

        public NodeMainData StartNode
        {
            get => graphData.nodes.Find(node => node.nodeType == NodeType.Input);
        }

        public IEnumerator Run()
        {
            var currentNode = StartNode;

            while (true)
            {
                yield return RunCoroutine(currentNode.coroutineData, target);

                var action = CreateAction(currentNode.actionData);
                yield return RunAction(action);

                if (currentNode.nodeType == NodeType.Output) yield break;
                currentNode = GetNextNode(currentNode);
            }
        }

        private IEnumerator RunCoroutine(NodeCoroutineData data, UnityEngine.MonoBehaviour target)
        {
            foreach (var coroutine in data.coroutines)
            {
                if (coroutine.isWaitingUntilFinish)
                {
                    yield return target.StartCoroutine(ExecuteCoroutine(coroutine));
                }
                else
                {
                    target.StartCoroutine(ExecuteCoroutine(coroutine));
                }
            }
        }

        private IEnumerator ExecuteCoroutine(CoroutineUnit unit)
        {
            switch (unit)
            {
                case WaitUnit waitUnit:
                    yield return waitUnit.Execute();
                    break;
                case ShowCharacterUnit showCharacterUnit:
                    yield return showCharacterUnit.Execute(characterArea, characters);
                    break;
                case ChangeBackgroundUnit changeBackgroundUnit:
                    yield return changeBackgroundUnit.Execute(backgroundArea, lastBackground);
                    break;
                case DeleteCharacterUnit deleteCharacterUnit:
                    yield return deleteCharacterUnit.Execute(characters);
                    break;
                case DeleteAllCharacterUnit deleteAllCharacterUnit:
                    yield return deleteAllCharacterUnit.Execute(characters);
                    break;
                default:
                    if (AddCustomCoroutines != null)
                    {
                        foreach (CoroutineDelegate coroutine in AddCustomCoroutines.GetInvocationList())
                        {
                            yield return coroutine(unit);
                        }
                    }
                    break;
            }
        }

        private IEnumerator RunAction(ActionPlayer action)
        {
            switch (action)
            {
                case TextDisplayer textDisplayer:
                    onInteraction += textDisplayer.Interaction;
                    yield return textDisplayer.Execute(nameTarget, subNameTarget, scriptTarget);
                    break;
                case SelectionDisplayer selectionDisplayer:
                    break;
            }

            onInteraction = null;
        }

        private NodeMainData GetNextNode(NodeMainData node)
        {
            var nextConnection = node.actionData.connections[0];
            return nodes[nextConnection];
        }
        
        private ActionPlayer CreateAction(NodeActionData actionData)
        {
            switch (actionData.actionNodeType)
            {
                case ActionType.CHARACTER:
                    return new TextDisplayer(actionData);
                case ActionType.USER:
                    return new SelectionDisplayer(actionData);
                default:
                    return new ActionPlayer(actionData);
            }
        }
    }
}