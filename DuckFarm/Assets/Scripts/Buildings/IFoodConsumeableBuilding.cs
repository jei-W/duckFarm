using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IFoodConsumeableBuilding
{
    bool FoodIsFull();
    bool FoodIsEmpty();
    bool InputFood( Food targetObject );
    Food GetFood();
    void BecameRottenFood( Food targetFood );
}
