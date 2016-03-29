image = screen.getScreen ();
sleep (60);
change = screen.getScreen ();

image.beginFilter ();
image.colorFilter (65, 148, 145, 190, 100, 255);
image.endFilter ();

change.beginFilter ();
change.colorFilter (65, 148, 145, 190, 100, 255);
change.endFilter ();

image.save ("first.png");
change.save ("second.png");

count = 1;
blobs = image.animation (change, 3, 10);

rect = nil;

for i, blob in ipairs (blobs) do
	change.drawRectangle (blob);
	targetX = blob.x + (blob.width / 2);
	targetY = blob.Y + (blob.height / 2);

	mouse.move (targetX, targetY);
	mouse.click (false);
	break;
end



change.save ("compare.png");

return false;