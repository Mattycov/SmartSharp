fishing = require "fishing"

spots = fishing.fishingSpots ();
for i, blob in ipairs (spots) do	
	print (blob);
	targetX = blob.x + (blob.width / 2);
	targetY = blob.y + (blob.height / 2);
	mouse.move (targetX, targetY);
end

sleep(500);

image = screen.getScreen ();
--child = image.child (SmartRectangle.new (395, 275, 136, 33));
child = image.child (SmartRectangle.new (4, 4, 350, 25));

child.save("child.png");
ocr.init ();

text = ocr.text (child, "UpChars07_s");
print (text);


image.drawRectangle (SmartRectangle.New (395, 275, 136, 33));
image.drawRectangle (SmartRectangle.New (236, 96, 49, 95));

image.save("screenshot.png");

return false;