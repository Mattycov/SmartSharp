fishing = require "fishing";
login = require "login";

screenShot = screen.getScreen ();

spots = fishing.fishingSpots ();
for i, blob in ipairs (spots) do
	targetX = blob.x + (blob.width / 2);
	targetY = blob.y + (blob.height / 2);
	mouse.move (targetX, targetY);
	break;
end

print(login.existingText());

screenShot.drawRectangle (SmartRectangle.New (236, 96, 49, 95));

screenShot.save("screenshot.png");

return false;