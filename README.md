# LearningSemanticKernel
I will try different approaches with microsoft semantic kernel.

To run the application we just need to simply copy the Anthropic API Key and put it into the appsettings.developement.json file.
What currently we have here: <br />
    - Chat completion. As Semantic Kernel doesn't have a separate connector for Anthropic models, we have used
      Microsoft.Extensions.AI and Anthropic Sdk support to get it inside semantic kernel.
