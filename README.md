# example_projectprime
A selection of the C# code used in Project Prime.

<b>ActionParseInputs</b> is the script that takes values from the New Input System's actions and parses the left and right controllers to the cannon-side and hand-side controls depending on the RightHanded toggle. It features custom scripts to parse axis inputs such as triggers and to simulate timed vibrations for devices that do not natively support it.

<b>ActionPlayerControl</b> is the main control script of the player that keeps the cannon-side and hand-side controls in separate functions to be handled by the correct controller from the input system.

<b>PlayerHands</b> keeps track of the handedness of the input system and moves the models into place accordingly. This is done via Rigidbodies and AddForce so that the hands become physical objects that cannot clip through walls.

<b>WeaponSystem</b> is called from ActionPlayerControl and handles the behaviour of each weapon type. It holds the current weapon being used, contains which weapons are unlocked, and organises a variable weapon select - when holding the right grip, the right stick angle is compared to the angles of each segment in a circle to determine which weapon to swap to.
