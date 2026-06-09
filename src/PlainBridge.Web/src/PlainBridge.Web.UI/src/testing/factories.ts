import { HostApplicationDto, ServerApplicationDto, UserDto } from '../app/models';

function randInt(min: number, max: number) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}
function uuid() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
    const r = Math.random() * 16 | 0;
    const v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

export function hostApp(overrides: Partial<HostApplicationDto> = {}): HostApplicationDto {
  return {
    id: randInt(1, 10000),
    appId: uuid(),
    name: `HostApp-${randInt(100, 999)}`,
    domain: `example${randInt(1, 99)}.local`,
    internalUrl: `http://svc${randInt(1, 99)}.local/api`,
    isActive: 1,
    ...overrides
  } as HostApplicationDto;
}

export function serverApp(overrides: Partial<ServerApplicationDto> = {}): ServerApplicationDto {
  return {
    id: randInt(1, 10000),
    appId: uuid(),
    name: `ServerApp-${randInt(100, 999)}`,
    serverApplicationAppId: uuid(),
    internalPort: randInt(1000, 60000),
    isActive: 1,
    ...overrides
  } as ServerApplicationDto;
}

export function user(overrides: Partial<UserDto> = {}): UserDto {
  return {
    id: randInt(1, 10000),
    username: `user${randInt(1000, 9999)}`,
    email: `user${randInt(1000, 9999)}@example.local`,
    name: `Name${randInt(1, 99)}`,
    family: `Family${randInt(1, 99)}`,
    phoneNumber: `+1-555-${randInt(100, 999)}-${randInt(1000, 9999)}`,
    ...overrides
  } as UserDto;
}

