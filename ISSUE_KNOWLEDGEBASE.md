# AI Agent Chat Project – Issue Knowledgebase

This document catalogs common issues encountered during development and deployment of the AI Agent Chat full-stack application (React frontend + .NET backend), along with their solutions. Use this as a quick reference for future debugging and DevOps work.

---

## 1. **Environment Variables Not Injected or Read Correctly**
**Symptoms:**
- Frontend or backend does not pick up environment variables (e.g., API URLs, connection strings).

**Solutions:**
- For React, prefix variables with `REACT_APP_` and ensure they are set at build time.
- For .NET backend, use `IConfiguration` to read variables. Set them in Azure App Service under Configuration > Application settings.

---

## 2. **CORS Errors Between Frontend and Backend**
**Symptoms:**
- Browser blocks API requests with CORS errors.

**Solutions:**
- In `Program.cs`, configure CORS to allow both local and deployed frontend origins:
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowReactApp",
          policy => policy
              .WithOrigins("http://localhost:3000", "https://<your-frontend-url>")
              .AllowAnyHeader()
              .AllowAnyMethod());
  });
  ...
  app.UseCors("AllowReactApp");
  ```

---

## 3. **API Endpoint Routing Issues (404 or Static File Handler)**
**Symptoms:**
- Azure App Service returns 404 or serves static files for API endpoints (e.g., `/api/chat/thread`).
- Error page shows handler as `StaticFile` and physical path points to a file, not a DLL.

**Solutions:**
- Ensure you are deploying the **publish output** (not source files) to Azure App Service.
- The publish output must include `web.config`, DLLs, and all runtime files.
- Project file (`.csproj`) must use `Microsoft.NET.Sdk.Web` and include:
  ```xml
  <AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>
  ```
- Deploy the contents of the publish folder to the root of the App Service (`site/wwwroot`).

---

## 4. **web.config Missing from Publish Output**
**Symptoms:**
- No `web.config` in the publish folder after `dotnet publish`.

**Solutions:**
- Change the SDK in `.csproj` to `Microsoft.NET.Sdk.Web`.
- Clean and republish:
  ```sh
  dotnet clean
  dotnet publish -c Release -o ./publish
  ```

---

## 5. **GitHub Actions Workflow Deploys Wrong Files**
**Symptoms:**
- Azure App Service does not run the latest code, or routing issues persist after deployment.

**Solutions:**
- Ensure the workflow publishes to a local folder (e.g., `myapp`), uploads that as an artifact, and deploys from that folder:
  ```yaml
  - name: dotnet publish
    run: dotnet publish agent_with_tool_V0 -c Release -o myapp
  - name: Upload artifact for deployment job
    uses: actions/upload-artifact@v4
    with:
      name: .net-app
      path: myapp
  ...
  - name: Download artifact from build job
    uses: actions/download-artifact@v4
    with:
      name: .net-app
  - name: Deploy to Azure Web App
    uses: azure/webapps-deploy@v3
    with:
      app-name: 'agent-chat'
      slot-name: 'Production'
      package: myapp
      publish-profile: ${{ secrets.AZUREAPPSERVICE_PUBLISHPROFILE }}
  ```

---

## 6. **Root Endpoint Not Accessible in Azure**
**Symptoms:**
- `/` returns "You do not have permission to view this directory or page." or similar.

**Solutions:**
- Ensure the backend is running as an ASP.NET Core app (see above for publish and deploy steps).
- Confirm `app.MapGet("/", ...)` is present in `Program.cs` and deployed.
- Confirm `web.config` is present in the deployed folder.

---

## 7. **Swagger UI Not Accessible**
**Symptoms:**
- `/swagger` returns 404 or is not found in Azure.

**Solutions:**
- Ensure `app.UseSwagger(); app.UseSwaggerUI();` are present in `Program.cs`.
- Confirm Swagger is enabled in all environments, not just Development.
- Confirm deployment includes all published files.

---

## 8. **General Debugging Tips**
- Use Azure App Service **Log stream** for real-time logs.
- Use **Advanced Tools > Go (Kudu)** to browse deployed files in `site/wwwroot`.
- Always restart the App Service after deployment.
- Test endpoints directly in the browser or with tools like Postman.

---

## 9. **Frontend API URL Issues**
**Symptoms:**
- Frontend cannot reach backend, or API URL is malformed.

**Solutions:**
- Ensure `REACT_APP_API_URL` is set correctly in the frontend environment.
- In code, remove trailing slashes before concatenating paths:
  ```js
  const apiUrl = (process.env.REACT_APP_API_URL || 'http://localhost:5000').replace(/\/+$/, '');
  ```

---

## 10. **Null Reference Warnings in Backend**
**Symptoms:**
- Warnings about possible null reference for connection strings or IDs.

**Solutions:**
- Ensure all required environment variables are set in Azure App Service.
- Add null checks or fallback values in code if needed.

---

## 11. **Frontend JavaScript Errors and API 500s/403s**
**Symptoms:**
- Console shows:
  - `Uncaught (in promise) TypeError: Cannot convert undefined or null to object at Object.keys`
  - `Uncaught (in promise) SyntaxError: Failed to execute 'json' on 'Response': Unexpected end of JSON input`
  - Network error: `Failed to load resource: the server responded with a status of 500 (Internal Server Error)` or 403 (Forbidden) from API endpoints.

**Solutions:**
- For 500 errors: Check backend logs for the cause and ensure the endpoint always returns valid JSON. Add error handling and logging in the backend controller.
- For 403 errors: Check CORS policy, authentication/authorization settings, and App Service access restrictions. Ensure the frontend origin is allowed and any required API keys or tokens are present.
- In the frontend, add error handling for failed fetch requests and invalid JSON responses. Always check for `null` or `undefined` before accessing object properties or using `Object.keys()`.

---

*Keep this file updated as new issues and solutions are discovered!*
