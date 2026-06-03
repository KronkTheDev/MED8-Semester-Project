using UnityEngine;

public class MenuBlock : MonoBehaviour
{
    public enum BlockType { ConditionA, ConditionB }
    
    [Header("Which block is this?")]
    public BlockType type;

    private VariantSelector selectorManager;

    void Start()
    {
        // Find the main menu manager in your scene automatically
        selectorManager = Object.FindFirstObjectByType<VariantSelector>();
    }

    // Fires instantly when your hand tracker's collider physical touches the block
    void OnTriggerEnter(Collider other)
    {
        // Check if the thing touching it is a hand interactor or controller
        if (other.name.Contains("Hand") || other.name.Contains("Controller") || other.name.Contains("Direct"))
        {
            TriggerGameStart();
        }
    }

    // Backup: Fires if you grab/collide using non-trigger physics colliders
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Hand") || collision.gameObject.name.Contains("Controller"))
        {
            TriggerGameStart();
        }
    }

    private void TriggerGameStart()
    {
        if (selectorManager == null) return;

        if (type == BlockType.ConditionA)
        {
            selectorManager.SelectConditionA();
        }
        else
        {
            selectorManager.SelectConditionB();
        }
    }
}