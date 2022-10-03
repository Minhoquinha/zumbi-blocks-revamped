1. Load `Modeltest` scene in your Unity project;

2. Create an empty scene object called `MyMeleeWeapon` and place it on the last spot of the row filled with weapons (the coordinates are  `37 6 0` for X, Y and Z respectively);

3. In this empty object, you will add the following components:
- Box Collider;
- Rigidbody;
- Melee;
- Item;

4. Now you will add the model and animations. After setting up the import configurations, add the `MyMeleeWeapon.blend` model into the `MyMeleeWeapon` object in the scene as a child object. Rename this child object `Model` to avoid confusions (use `Models` if there are child models in it);

5. Create a new empty object with the name of `ArmsMyMeleeWeapon`. Add the `ArmsMyMeleeWeapon.blend` animations into this new object as a child object. Rename this child object as `Models` to avoid confusions again. Now, there is an empty parent with it's child being the full set of animation models named `ArmsMyMeleeWeapon`, make this parent's layer `UI` (as well as all children). From this point on each child will be called it's parent's name following it's own name separated by `\` (example: `ArmsMyMeleeWeapon\Models` for the animation models). Grab this whole `ArmsMyMeleeWeapon` object and put it as a child of the whole `MyMeleeWeapon` object (NOT as a child of `MyMeleeWeapon\Model`);

6. Now you will scale the objects. The animations have correct proportions so you will use them as base. Rescale the `MyMeleeWeapon\ArmsMyMeleeWeapon\Models` to `3.5 3.5 3.5` for X, Y and Z respectively. You shouldn't need to reposition/rerotate it and should keep all those `Transform` values as `0`;

7. Using the animations as the base of what `MyMeleeWeapon` should look like, resize it's child `MyMeleeWeapon\Model` (or `MyMeleeWeapon\Models` if you used that in step 4) to make it as close as possible to the size it is on the player's hand. After resizing, rotate the object to make it have the same direction and way of the other melee weapons in the `Modeltest` scene;

8. Now it's time to rescale the collider. Simply change the `Size` variables of `BoxCollider` component until they make a box around the object that is close as possible to it's borders while still being on the outside;

9. Save the weapon as a prefab in the `Assets\Prefabs` folder as the hardest part is over and you will need references to it's prefab later;

10. Now is the fun part, setting up the weapon's stats in `Melee`. The `MeleeName` is `MyMeleeWeapon` and `PlayerDependant` should remain `false`. The Combat Stats can be chosen to what you desire for this weapon, remember melee weapons can't hit through walls so `Penetration` only affects their damage output through armor. `FixedAttackDelay` should be used to make the attack synchronize with the weapon's animations. `Spread` is `0` by default for all melee weapons. Use other melee weapons as references for changing these stats, remembering that normal zombies and players have `100` health. Change the references for `ModelUnequipped` and `ModelEquipped` for `MyMeleeWeapon\Model`/`MyMeleeWeapon\Models` and `MyMeleeWeapon\ArmsMyMeleeWeapon` respectively. `ImpactEffectsCollection` should reference `ImpactEffects.asset` inside of `Assets\Editor`;

11. Time to change the values inside of the `Item` component. Change the `ItemName` to the name of the weapon (`MyMeleeWeapon`), the prefab to it's prefab reference inside the `Assets\Prefabs` folder, and the `ItemAmount` and `ItemType` to `1` and `1` respectively (the first is the amount of items inside that object, only useful to disposables so anything else is 1, and the later is for the type of this object, in this case a melee weapon). The `ItemUI` variable should reference an `ItemUI` object prefab inside `Assets\Prefabs\UI`, which is the same for all items. The `UIRenderDistance` should remain `50` by default and the `PickUpSoundClip` should reference `grab.wav` inside `Assets\Sounds\Guns\Reload`. Add the `MyMeleeWeapon` object into the collection list `Assets\Editor\Items.asset`, the ID number is set automatically after that;

12. Finally, change the mass of the object in the Rigidbody component to something that makes sense, usually less than 0.3 units. Make sure the object and all of it's children (except MyMeleeWeapon\ArmsMyMeleeWeapon) are in the `Item` layer;

13. Save the prefab once more, save the scene and test the weapon for a bit by loading the `GameTest1` scene and placing the weapon in the map. Then just pick it up and play a bit with it.