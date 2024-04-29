using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;

namespace RainbowAssets.BehaviourTree.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        public const string path = "Assets/Asset Packs/Rainbow Assets/Scripts/Behaviour Tree/Editor/";
        BehaviourTreeView behaviourTreeView;

        [MenuItem("Rainbow Assets/Behaviour Tree Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BehaviourTreeEditor), false, "Behaviour Tree Editor");
        }

        [OnOpenAsset]
        public static bool OnBehaviourTreeOpened(int instanceID, int line)
        {
            BehaviourTree behaviourTree = EditorUtility.InstanceIDToObject(instanceID) as BehaviourTree;

            if (behaviourTree != null)
            {
                ShowWindow();
                return true;
            }

            return false;
        }

        void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            behaviourTreeView = root.Q<BehaviourTreeView>();
        }

        void OnSelectionChange()
        {
            BehaviourTree behaviourTree = Selection.activeObject as BehaviourTree;

            if (Selection.activeGameObject)
            {
                BehaviourTreeController controller = Selection.activeGameObject.GetComponent<BehaviourTreeController>();

                if (controller != null)
                {
                    behaviourTree = controller.GetBehaviourTree();
                }
            }

            if (behaviourTree != null)
            {
                behaviourTreeView.Refresh(behaviourTree);
            }
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (behaviourTreeView != null)
            {
                if (change == PlayModeStateChange.EnteredEditMode)
                {
                    OnSelectionChange();
                }

                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    OnSelectionChange();
                }
            }
        }

        void OnInspectorUpdate()
        {
            if (behaviourTreeView != null)
            {
                behaviourTreeView.DrawStatus();
            }
        }
    }
}