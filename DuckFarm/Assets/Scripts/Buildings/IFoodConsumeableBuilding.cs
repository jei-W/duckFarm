using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IFoodConsumeableBuilding
{
    bool FoodIsFull();
    bool FoodIsEmpty();
    void InputFood( Food targetObject );
    Food GetFood();
}
