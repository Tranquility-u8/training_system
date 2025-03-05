using Mujoco;
using UnityEngine;

public class UTScene : MjScene
{
    public new static MjScene Instance
    {
        get => MjScene.Instance;
    }
    
}