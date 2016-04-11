textures = require "rs3/textures";

someTextures = textures.texture (88625);
print(#someTextures);

if #someTextures > 0 then
	debug = screen.getDebug ();
	for i, texture in ipairs(someTextures) do
		rectangle = SmartRectangle.new (texture.x1, texture.y1, texture.x2 - texture.x1, texture.y2 - texture.y1);
		debug.drawRectangle (rectangle);
		mouse.move(texture.x, texture.y);
		sleep(1000);
	end	
	screen.setDebug(debug);
	debug.save("debug.png");
end

return false;