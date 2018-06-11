using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Unity;
#if UNITY_WEBGL
using PlayGen.SUGAR.Unity.WebGL;
#endif

public class PlatformSUGARUnityManager : SUGARUnityManager
{
    protected override IHttpHandler CreateHttpHandler()
    {
#if UNITY_WEBGL
        return new UnityWebGlHttpHandler();
#else
        return null;
#endif
    }
}