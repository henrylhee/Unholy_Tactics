//UNITY_SHADER_NO_UPGRADE
#ifndef MovingCircle_INCLUDED
#define MovingCircle_INCLUDED

void MovingCircle_float(float2 _in, float thickness, float maxRadius, float segmentSize,float lineWidth, float time, float opacity, out float Out)
{
    float currentRadius = 0;
    float actualMaxRadius = maxRadius * (lineWidth/2) * time;
    float actualThickness = thickness * (lineWidth/2);

    float2 d = _in;
    float2 center;
    center.x = 1 - (segmentSize/2);
    center.y = 0.5;
    float2 v = d - center;
    v.x = v.x/segmentSize;
    float lengthV = length(v); 


    if(lengthV <= actualMaxRadius && lengthV > actualMaxRadius - actualThickness)
    {
        Out = 1 * opacity;
    }
    else
    {
        Out = 0;
    }
}
#endif //MovingCircle_INCLUDED
