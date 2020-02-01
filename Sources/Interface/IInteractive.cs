using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractive
{
    void Interact(Player pController);
    void StopInteraction();

    void Begin();
    void End();

	bool CanInteract(Player pController);

    void CancelInteraction();
}
