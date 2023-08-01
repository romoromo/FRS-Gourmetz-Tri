import { AppPage } from './app.po';

describe('FRS App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display application title: FRS', () => {
    page.navigateTo();
    expect(page.getAppTitle()).toEqual('FRS');
  });
});
