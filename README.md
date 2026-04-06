# im_back_baby

## Gemini API Key Setup
To use the Gemini AI features, you need an API key from Google AI Studio.

### Secure API Key Handling:
1.  **Do not** enter the key directly into the `GeminiAICreds` ScriptableObject if you intend to commit your changes to a public repository.
2.  Create a file named `gemini.key` in the project root directory (the same level as `Assets` and `ProjectSettings`).
3.  Paste your Gemini API key into this file.
4.  The `AiSetting` ScriptableObject will automatically load the key from this file at runtime if its `apiKey` field in the asset is empty.
5.  The `.gitignore` has been updated to ignore `*.key` files to prevent accidental leakage.