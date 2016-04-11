local debug = {};

debugImage = screen.getDebug();

function debug.begin()
  debugImage = SmartImage.blank(800, 600);
end

function debug.finish()
  screen.setDebug(debugImage);
end

function debug.drawLine(startPoint, endPoint, colour)
  debugImage.drawLine(startPoint, endPoint, colour);
end

function debug.drawRectangle(rectangle, colour)
  debugImage.drawRectangle(rectangle, colour);
end

function drawString(string, location, colour)
  debugImage.drawString(string, location, colour);
end

return debug;
