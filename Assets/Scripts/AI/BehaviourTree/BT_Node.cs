using System.Collections.Generic;

public class Node
{
    public Node parent;
    protected List<Node> children = new List<Node>();

    public Node()
    {
        parent = null;
    }
    public Node(List<Node> children)
    {
        foreach (Node n in children)
        {
            _Attach(n);
        }
    }
    public void _Attach(Node node)
    {
        node.parent = this;
        children.Add(node);
    }
    public virtual void Evaluate()
    {
        //TODO: NODE EVALUATION
    }
}
public class Sequence : Node
{
    public override void Evaluate()
    {
        base.Evaluate();

        foreach (Node child in children)
        {
            child.Evaluate();
        }
    }
}