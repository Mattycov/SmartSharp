textures = require "rs3/textures";

local login = {};

function login.atLogin()
  loginTexture = textures.texture(6887954);
  return #loginTexture > 0;
end

function login.remember()
  if login.atLogin() then
    rememberTexture = textures.texture(4649);
    return #rememberTexture > 0
  end
  return false;
end

return login;
