# HAP Rider AC Combat (Holsters) with UMA 

## Usage - Quick How to guide

### Quick Setup:

1. Clone this repo into your Assets folder. All of its assets are now within HAPRiderUMA folder.
2. After Unity re-compiles, it should display a Debug message complaining about UMARider.cs that is trying to use an inaccessible set accessor. You will need to open 'Assets\Malbers Animations\Common\Scripts\Riding System\Rider\MRider.cs' and scroll to line 264 and 266, then remove private from the set accessors on both lines. Save and have Unity recompile.
3. Pull in the Prefabs from this repo into a scene (Steve Inventory will need to be added to the InventorySystemDemo scene).

### Creating UMA Inventory Items (For Malbers Inventory System):

1. To create a new UMA Clothing Item use the Create menu:
  - Assets -> Create -> Malbers Inventory -> New UMA Clothing Item
2. On the new Item's General Tab:
  - Fill in the Item's details
  - Set an UMA Text Recipe
  - Tick Equippable
3. On the Item's Reactions Tab:
  - Change the Equip Reaction to the "UMA/Equip Clothing Item" reaction
  - Change the Unequip Reaction to the "UMA/Unequip Clothing Item" reaction
4. You will also need to create a Prefab for the Item:
  - Copy an existing Item in the InventorySystemDemo scene
  - Change it's Inventory Item property to your new UMA Clothing Item under the Inventory Item Component.
  - Turn it into a Prefab
  - Set this new Prefab as your UMA Clothing Item's In World Prefab.

### Creating your own Rider prefab:

1. First follow this tutorial to get HAP and UMA working together.
https://www.youtube.com/watch?v=mlYu0YQojHA

2. Change the Animation Controller on your new Rider to the "AC Human v4 Rider". There's 3 places to do this:
  - Animator component - Controller field
  - Dynamic Character Avatar component - Default Animation Controller field (Under Race Animation Controllers)
  - Dynamic Character Avatar component - Animation Controller field (Under Advanced Options)

3. Add the UMARider script as Component to your Rider.

4. Add the Created event.
  - Open Character Events on the Dynamic Character Avatar component.
  - Click + under the Character Created list.
  - Drag the UMARider component into the new event's Object field.
  - Under the event's Function dropdown, select UMARider.UpdateRiderHandBones().

5. Remove the Weapon Manager compoent's holster slots.
  - Under the Weapon Manager component -> Holsters tab, make sure 'Use Holsters' is selected.
  - In the Holster list you should see a list containing Left, Right and Back holsters. Select each one and remove their Slots as follows:
  - Select a Holster. Its properties will appear just below the Holster's list that contains a Slots list.
  - Remove the default Slots from this list and repeat for all Holsters.

## Known Issues:

1. The Aiming is off. This could help: [Fixing Bow Aim At Runtime Offset IK - Discord](https://discord.com/channels/640979038449696770/664963793876418600/1078374863653322782)
2. The UMARider has some default offsets and rotations for the equip points and holsters. They need to be tweeked.
3. The UMARider script should allow you to set weapons that your UMA Rider should start with. These are not working at the moment.
