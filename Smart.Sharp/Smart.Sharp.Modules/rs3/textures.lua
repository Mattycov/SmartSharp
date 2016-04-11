local textures = {};

function textures.texture(id)
  textureArray = glx.textures();
  result = {};
  for i, texture in ipairs(textureArray) do
    if texture.id == id then
      table.insert(result, texture);
    end
  end
  return result;
end

function textures.textures(ids)
  textureArray = glx.textures();
  result = {};
  for i, texture in ipairs(textureArray) do
    for i, id in ipairs(ids) do
      if texture.id == id then
        table.insert(result, texture);
      end
    end
  end
  return result;
end

return textures;
