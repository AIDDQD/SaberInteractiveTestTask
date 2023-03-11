using SerializerTests.Nodes;

namespace SerializerTests.Implementations;

internal static class DeepCopier
{
    public static ListNode DeepCopy(ListNode head)
    {
        if (head == null) return null;

        var copiedNodes = new Dictionary<ListNode, ListNode>();

        var newHead = new ListNode();
        newHead.Data = head.Data;
        copiedNodes.Add(head, newHead);

        var currentNode = head.Next;
        var previousNode = newHead;
        while (currentNode != null)
        {
            var newNode = new ListNode();
            newNode.Data = currentNode.Data;
            copiedNodes.Add(currentNode, newNode);

            previousNode.Next = newNode;
            newNode.Previous = previousNode;

            previousNode = newNode;
            currentNode = currentNode.Next;
        }

        currentNode = head;
        while (currentNode != null)
        {
            if (currentNode.Random != null) copiedNodes[currentNode].Random = copiedNodes[currentNode.Random];

            currentNode = currentNode.Next;
        }

        return newHead;
    }
}