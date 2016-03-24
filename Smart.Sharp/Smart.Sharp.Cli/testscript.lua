image = screen.getScreen();
image.beginFilter();
image.colorFilter (255, 255, 255, 255, 0, 0);
image.endFilter ();

count = 1;
blobs = image.blobs();

for i, blob in ipairs (blobs) do
	child = image.child (blob);
	child.save(count .. ".png");
	count = count + 1;
end

return false;