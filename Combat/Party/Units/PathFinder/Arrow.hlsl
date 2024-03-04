//UNITY_SHADER_NO_UPGRADE
#ifndef Arrow_INCLUDED
#define Arrow_INCLUDED

void Arrow_float(float2 _in, float width, float height, float opacity, out float Out)
{
    float2 d = _in;
    d.x = 1-d.x; 
    float offX = (1-width)/2;
    float offY = (1-height)/2;
    float slope = (height/2)/width;
    float n = offY - slope*offX;

    if(d.y > offY && d.y < 1-offY && d.x > offX && d.x < 1-offX)
    {
        if(d.y > (slope * d.x + n) && d.y < 1 - (slope * d.x + n))
        {
            Out = opacity;
        }
        else
        {
            Out = 0;
        }
    }
    else
    {
        Out = 0;
    }
}
#endif //Arrow_INCLUDED
