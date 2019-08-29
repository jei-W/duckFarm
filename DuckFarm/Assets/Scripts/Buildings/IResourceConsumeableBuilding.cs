using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IResourceConsumeableBuilding
{
    bool ResourceIsFull();
    bool ResourceIsEmpty();
    bool InputResource( Resource targetObject );
    Resource GetResource( World.ResourceType type );
}
