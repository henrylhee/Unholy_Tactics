//UNITY_SHADER_NO_UPGRADE
#ifndef CircleRange_INCLUDED
#define CircleRange_INCLUDED

void CircleRange_float(float2 _in, float RangeX, float RangeY, float PosX, float PosY, out float Out)
{
    
    float2 v;
    v.x = abs(_in.x-PosX);
    v.y = abs(_in.y-PosY);

    if(v.x <= RangeX && v.y <= RangeY)
    {
        Out = 1;
    }
    else
    {
        Out = 0;
    }
}
#endif //CircleRange_INCLUDED
