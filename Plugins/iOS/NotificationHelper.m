//
//  NotificationHelper
//  Unity-iPhone
//
//  Created by Stephen Zhou on 13-8-5.
//
//

void _SetIconBadgeNumber(int num)
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = num;
}