namespace LobsterAdventure.Core.Models
{
    public class AdventureTreeNode
    {
        public AdventureTreeNode(int nodeId, string nodeText, bool isSelected = false, AdventureTreeNode? leftChild = null , AdventureTreeNode? rightChild = null)
        {
            NodeId = nodeId;
            NodeText = nodeText;
            IsSelected = isSelected;
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public int NodeId { get; set; }
        public string NodeText { get; set; }
        public bool IsSelected { get; set; }
        public AdventureTreeNode? LeftChild { get; set; }
        public AdventureTreeNode? RightChild { get; set; }
    }
}
