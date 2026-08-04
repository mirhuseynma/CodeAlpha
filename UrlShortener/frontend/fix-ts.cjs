const fs = require('fs');

const replaceInFile = (file, search, replace) => {
  const content = fs.readFileSync(file, 'utf8');
  fs.writeFileSync(file, content.replace(search, replace));
};

replaceInFile('src/components/ui/Button.tsx', 'import React, { ButtonHTMLAttributes } from \'react\';', 'import React, { type ButtonHTMLAttributes } from \'react\';');

replaceInFile('src/components/ui/Input.tsx', 'import React, { InputHTMLAttributes } from \'react\';', 'import React, { type InputHTMLAttributes } from \'react\';');

replaceInFile('src/features/auth/AuthContext.tsx', 'import React, { createContext, useContext, useState, useEffect, ReactNode } from \'react\';', 'import { createContext, useContext, useState, useEffect, type ReactNode } from \'react\';');

replaceInFile('src/features/auth/LoginForm.tsx', 'import React, { useState } from \'react\';', 'import { useState } from \'react\';');
replaceInFile('src/features/auth/LoginForm.tsx', 'import { loginSchema, LoginFormData } from \'./schemas\';', 'import { loginSchema, type LoginFormData } from \'./schemas\';');

replaceInFile('src/features/auth/RegisterForm.tsx', 'import React, { useState } from \'react\';', 'import { useState } from \'react\';');
replaceInFile('src/features/auth/RegisterForm.tsx', 'import { registerSchema, RegisterFormData } from \'./schemas\';', 'import { registerSchema, type RegisterFormData } from \'./schemas\';');

replaceInFile('src/features/links/CreateLinkForm.tsx', 'import React, { useState } from \'react\';', 'import { useState } from \'react\';');
replaceInFile('src/features/links/CreateLinkForm.tsx', 'import { createLinkSchema, CreateLinkFormData } from \'./schemas\';', 'import { createLinkSchema, type CreateLinkFormData } from \'./schemas\';');

replaceInFile('src/features/links/LinkResultPanel.tsx', 'import React, { useState } from \'react\';', 'import { useState } from \'react\';');
replaceInFile('src/features/links/LinkResultPanel.tsx', 'import { ShortLinkResponse } from \'./CreateLinkForm\';', 'import type { ShortLinkResponse } from \'./CreateLinkForm\';');

replaceInFile('src/main.tsx', 'import App from \'./App\' // We\'ll replace App with our routes, but keeping it as a layout could be useful.\n\n', '');

replaceInFile('src/pages/Dashboard.tsx', 'import React from \'react\';\n', '');
replaceInFile('src/pages/Home.tsx', 'import React from \'react\';\n', '');
replaceInFile('src/pages/Login.tsx', 'import React from \'react\';\n', '');
replaceInFile('src/pages/Register.tsx', 'import React from \'react\';\n', '');

replaceInFile('src/services/api.ts', 'import axios, { AxiosError, InternalAxiosRequestConfig } from \'axios\';', 'import axios, { type AxiosError, type InternalAxiosRequestConfig } from \'axios\';');

console.log('Done');
